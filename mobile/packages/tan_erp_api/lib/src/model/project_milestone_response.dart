//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_milestone_response.g.dart';

/// ProjectMilestoneResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [plannedDate] 
/// * [weight] 
/// * [sortOrder] 
/// * [completedAtUtc] 
/// * [completedBy] 
/// * [rowVersion] 
@BuiltValue()
abstract class ProjectMilestoneResponse implements Built<ProjectMilestoneResponse, ProjectMilestoneResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'plannedDate')
  Date? get plannedDate;

  @BuiltValueField(wireName: r'weight')
  int? get weight;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'completedAtUtc')
  DateTime? get completedAtUtc;

  @BuiltValueField(wireName: r'completedBy')
  ProjectPersonResponse? get completedBy;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  ProjectMilestoneResponse._();

  factory ProjectMilestoneResponse([void updates(ProjectMilestoneResponseBuilder b)]) = _$ProjectMilestoneResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectMilestoneResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectMilestoneResponse> get serializer => _$ProjectMilestoneResponseSerializer();
}

class _$ProjectMilestoneResponseSerializer implements PrimitiveSerializer<ProjectMilestoneResponse> {
  @override
  final Iterable<Type> types = const [ProjectMilestoneResponse, _$ProjectMilestoneResponse];

  @override
  final String wireName = r'ProjectMilestoneResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectMilestoneResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
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
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.completedAtUtc != null) {
      yield r'completedAtUtc';
      yield serializers.serialize(
        object.completedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.completedBy != null) {
      yield r'completedBy';
      yield serializers.serialize(
        object.completedBy,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectMilestoneResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectMilestoneResponseBuilder result,
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
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'completedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.completedAtUtc = valueDes;
          break;
        case r'completedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.completedBy.replace(valueDes);
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectMilestoneResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectMilestoneResponseBuilder();
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


