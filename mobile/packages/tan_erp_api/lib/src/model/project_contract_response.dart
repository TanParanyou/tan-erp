//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_contract_response.g.dart';

/// ProjectContractResponse
///
/// Properties:
/// * [baselineAmount] 
/// * [approvedDelta] 
/// * [currentAmount] 
@BuiltValue()
abstract class ProjectContractResponse implements Built<ProjectContractResponse, ProjectContractResponseBuilder> {
  @BuiltValueField(wireName: r'baselineAmount')
  double? get baselineAmount;

  @BuiltValueField(wireName: r'approvedDelta')
  double? get approvedDelta;

  @BuiltValueField(wireName: r'currentAmount')
  double? get currentAmount;

  ProjectContractResponse._();

  factory ProjectContractResponse([void updates(ProjectContractResponseBuilder b)]) = _$ProjectContractResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectContractResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectContractResponse> get serializer => _$ProjectContractResponseSerializer();
}

class _$ProjectContractResponseSerializer implements PrimitiveSerializer<ProjectContractResponse> {
  @override
  final Iterable<Type> types = const [ProjectContractResponse, _$ProjectContractResponse];

  @override
  final String wireName = r'ProjectContractResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectContractResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.baselineAmount != null) {
      yield r'baselineAmount';
      yield serializers.serialize(
        object.baselineAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.approvedDelta != null) {
      yield r'approvedDelta';
      yield serializers.serialize(
        object.approvedDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.currentAmount != null) {
      yield r'currentAmount';
      yield serializers.serialize(
        object.currentAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectContractResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectContractResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'baselineAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.baselineAmount = valueDes;
          break;
        case r'approvedDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.approvedDelta = valueDes;
          break;
        case r'currentAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.currentAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectContractResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectContractResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


