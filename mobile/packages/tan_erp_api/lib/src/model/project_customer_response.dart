//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_customer_response.g.dart';

/// ProjectCustomerResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [displayNameTh] 
/// * [displayNameEn] 
@BuiltValue()
abstract class ProjectCustomerResponse implements Built<ProjectCustomerResponse, ProjectCustomerResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  ProjectCustomerResponse._();

  factory ProjectCustomerResponse([void updates(ProjectCustomerResponseBuilder b)]) = _$ProjectCustomerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectCustomerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectCustomerResponse> get serializer => _$ProjectCustomerResponseSerializer();
}

class _$ProjectCustomerResponseSerializer implements PrimitiveSerializer<ProjectCustomerResponse> {
  @override
  final Iterable<Type> types = const [ProjectCustomerResponse, _$ProjectCustomerResponse];

  @override
  final String wireName = r'ProjectCustomerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameEn != null) {
      yield r'displayNameEn';
      yield serializers.serialize(
        object.displayNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectCustomerResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'displayNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameEn = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectCustomerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectCustomerResponseBuilder();
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


