//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'add_project_milestone_request.g.dart';

/// AddProjectMilestoneRequest
///
/// Properties:
/// * [name] 
/// * [plannedDate] 
/// * [weight] 
@BuiltValue()
abstract class AddProjectMilestoneRequest implements Built<AddProjectMilestoneRequest, AddProjectMilestoneRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'plannedDate')
  Date? get plannedDate;

  @BuiltValueField(wireName: r'weight')
  int? get weight;

  AddProjectMilestoneRequest._();

  factory AddProjectMilestoneRequest([void updates(AddProjectMilestoneRequestBuilder b)]) = _$AddProjectMilestoneRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AddProjectMilestoneRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AddProjectMilestoneRequest> get serializer => _$AddProjectMilestoneRequestSerializer();
}

class _$AddProjectMilestoneRequestSerializer implements PrimitiveSerializer<AddProjectMilestoneRequest> {
  @override
  final Iterable<Type> types = const [AddProjectMilestoneRequest, _$AddProjectMilestoneRequest];

  @override
  final String wireName = r'AddProjectMilestoneRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AddProjectMilestoneRequest object, {
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
  }

  @override
  Object serialize(
    Serializers serializers,
    AddProjectMilestoneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AddProjectMilestoneRequestBuilder result,
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AddProjectMilestoneRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AddProjectMilestoneRequestBuilder();
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


