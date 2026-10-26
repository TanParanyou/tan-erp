//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_project_milestone_request.g.dart';

/// UpdateProjectMilestoneRequest
///
/// Properties:
/// * [name] 
/// * [plannedDate] 
/// * [weight] 
/// * [expectedVersion] 
@BuiltValue()
abstract class UpdateProjectMilestoneRequest implements Built<UpdateProjectMilestoneRequest, UpdateProjectMilestoneRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'plannedDate')
  Date? get plannedDate;

  @BuiltValueField(wireName: r'weight')
  int? get weight;

  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  UpdateProjectMilestoneRequest._();

  factory UpdateProjectMilestoneRequest([void updates(UpdateProjectMilestoneRequestBuilder b)]) = _$UpdateProjectMilestoneRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateProjectMilestoneRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateProjectMilestoneRequest> get serializer => _$UpdateProjectMilestoneRequestSerializer();
}

class _$UpdateProjectMilestoneRequestSerializer implements PrimitiveSerializer<UpdateProjectMilestoneRequest> {
  @override
  final Iterable<Type> types = const [UpdateProjectMilestoneRequest, _$UpdateProjectMilestoneRequest];

  @override
  final String wireName = r'UpdateProjectMilestoneRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateProjectMilestoneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.plannedDate != null) {
      yield r'plannedDate';
      yield serializers.serialize(
        object.plannedDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.weight != null) {
      yield r'weight';
      yield serializers.serialize(
        object.weight,
        specifiedType: const FullType(int),
      );
    }
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateProjectMilestoneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateProjectMilestoneRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'plannedDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedDate = valueDes;
          break;
        case r'weight':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.weight = valueDes;
          break;
        case r'expectedVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateProjectMilestoneRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateProjectMilestoneRequestBuilder();
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


